import { useMemo, useRef, useState } from 'react';
import { Canvas, useThree } from '@react-three/fiber';
import { Grid, Line, OrbitControls, OrthographicCamera, PerspectiveCamera } from '@react-three/drei';
import { Color } from 'three';
import type { Rack, StorageLocation, WarehouseLayout } from '../../../util/Types.js';
import InstancedBoxes from './InstancedBoxes.js';
import TextSprite from './TextSprite.js';
import {
  fillRatio,
  locationBox,
  postBoxes,
  rackHeight,
  shelfBoxes,
  type Vec3,
} from './layoutGeometry.js';
import { fillColor, type ScenePalette } from './scenePalette.js';

export type LayoutViewMode = '3d' | 'top';

type WarehouseSceneProps = {
  layout: WarehouseLayout;
  mode: LayoutViewMode;
  palette: ScenePalette;
  selectedLocationId: number | null;
  highlightProductId: number | null; // ennek a terméknek a helyei kiemelve, a többi foglalt hely halványan
  onSelect: (locationId: number | null) => void;
};

type PlacedLocation = { rack: Rack; location: StorageLocation };

// A polc kódja a polc elejénél (az 1. rekesz fölött), a folyosó felőli oldalon: háttal álló polcpárnál
// így a két felirat nem takarja egymást. "Hátoldal" az, ahol közvetlenül egy másik polc áll.
function rackLabelPosition(rack: Rack, racks: Rack[]): Vec3 {
  const horizontal = rack.orientation === 'Horizontal';
  const cells = (r: Rack) => Array.from({ length: r.length }, (_, i) =>
    (r.orientation === 'Horizontal' ? `${r.x + i},${r.y}` : `${r.x},${r.y + i}`));
  const occupied = new Set(racks.filter((r) => r.id !== rack.id).flatMap(cells));
  const backIsBefore = horizontal ? occupied.has(`${rack.x},${rack.y - 1}`) : occupied.has(`${rack.x - 1},${rack.y}`);
  const offset = backIsBefore ? 1.15 : -0.15; // a cellán belül: a túloldal a folyosó

  const y = rackHeight(rack) + 0.25;
  return horizontal ? [rack.x + 0.5, y, rack.y + offset] : [rack.x + offset, y, rack.y + 0.5];
}

// Kamera és vezérlés: 3D-ben perspektivikus, körbeforgatható nézet; felülnézetben ortografikus alaprajz
// (fent a raktár hátsó fala, lent a depó), csak mozgatással és nagyítással.
function SceneCamera({ mode, width, depth }: { mode: LayoutViewMode; width: number; depth: number }) {
  const size = useThree((state) => state.size);
  const target: Vec3 = [width / 2, 0, depth / 2];
  const span = Math.max(width, depth);

  if (mode === '3d') {
    // Akkora távolság, hogy az alaprajz a vászon arányától függetlenül beférjen; 55°-os rálátással elölről
    const fov = 45;
    const halfV = (fov / 2) * (Math.PI / 180);
    const halfH = Math.atan(Math.tan(halfV) * (size.width / Math.max(1, size.height)));
    const distance = 1.15 * Math.max(width / 2 / Math.tan(halfH), depth / 2 / Math.tan(halfV)) + 4;
    const elevation = 55 * (Math.PI / 180);
    const position: Vec3 = [width / 2, distance * Math.sin(elevation), depth / 2 + distance * Math.cos(elevation)];

    return (
      <>
        <PerspectiveCamera makeDefault fov={fov} near={0.1} far={span * 10} position={position} />
        <OrbitControls key="3d" makeDefault target={target} maxPolarAngle={Math.PI / 2.05} minDistance={3} maxDistance={distance * 2.5} />
      </>
    );
  }

  const zoom = Math.min(size.width / (width + 2), size.height / (depth + 2));
  return (
    <>
      <OrthographicCamera makeDefault near={0.1} far={200} position={[width / 2, 50, depth / 2]} up={[0, 0, -1]} zoom={zoom} />
      <OrbitControls key="top" makeDefault target={target} enableRotate={false} screenSpacePanning />
    </>
  );
}

// A raktár 3D modellje: padló, falak, depó, polcok és a tárolóhelyek telítettség szerint színezett dobozai.
// Kattintásra a hely kiválasztható (onSelect), az egér alatti hely kódja a vászon sarkában látszik.
export default function WarehouseScene({
  layout,
  mode,
  palette,
  selectedLocationId,
  highlightProductId,
  onSelect,
}: WarehouseSceneProps) {
  const [hovered, setHovered] = useState<PlacedLocation | null>(null);
  const pointerDown = useRef<{ x: number; y: number } | null>(null);
  const { width, depth } = layout;

  const placed = useMemo(
    () => layout.racks.flatMap((rack) => rack.locations.map((location) => ({ rack, location }))),
    [layout],
  );
  const stocked = useMemo(() => placed.filter((p) => p.location.quantity > 0), [placed]);
  const empty = useMemo(() => placed.filter((p) => p.location.quantity === 0), [placed]);

  const rackBoxes = useMemo(
    () => ({ shelves: layout.racks.flatMap(shelfBoxes), posts: layout.racks.flatMap(postBoxes) }),
    [layout],
  );
  const stockedBoxes = useMemo(() => stocked.map((p) => locationBox(p.rack, p.location)), [stocked]);
  const emptyBoxes = useMemo(() => empty.map((p) => locationBox(p.rack, p.location)), [empty]);

  const stockedColors = useMemo(() => {
    const selected = new Color(palette.selected);
    const highlight = new Color(palette.highlight);
    const dimmed = new Color(palette.emptyBin);
    return stocked.map(({ location }) => {
      if (location.id === selectedLocationId) return selected;
      if (highlightProductId === null) return fillColor(palette, fillRatio(location));
      return location.stocks.some((s) => s.productId === highlightProductId) ? highlight : dimmed;
    });
  }, [stocked, palette, selectedLocationId, highlightProductId]);

  const emptyColors = useMemo(() => {
    const selected = new Color(palette.selected);
    const normal = new Color(palette.emptyBin);
    return empty.map(({ location }) => (location.id === selectedLocationId ? selected : normal));
  }, [empty, palette, selectedLocationId]);

  const outline: Vec3[] = [[0, 0.02, 0], [width, 0.02, 0], [width, 0.02, depth], [0, 0.02, depth], [0, 0.02, 0]];
  const dockPosition: Vec3 = [layout.dockX + 0.5, 0.015, layout.dockY + 0.5];

  // A kamera húzása után felengedett egér ne törölje a kijelölést
  const handleMissed = (event: MouseEvent) => {
    const start = pointerDown.current;
    if (start && Math.hypot(event.clientX - start.x, event.clientY - start.y) > 4) return;
    onSelect(null);
  };

  const label = { color: palette.labelText, background: palette.labelBackground, border: palette.labelBorder };
  // 3D-ben a feliratok a távolságtól függetlenül olvashatók maradnak; felülnézetben a rajzzal együtt nagyítódnak
  const labelSize = mode === '3d' ? { fixedSize: true, height: 0.045 } : { height: 0.7 };

  return (
    <div className="layout3d-scene">
      <Canvas
        dpr={[1, 2]}
        onPointerDown={(event) => { pointerDown.current = { x: event.clientX, y: event.clientY }; }}
        onPointerMissed={handleMissed}
        style={{ cursor: hovered ? 'pointer' : 'grab' }}
      >
        <color attach="background" args={[palette.background]} />
        <SceneCamera mode={mode} width={width} depth={depth} />

        <ambientLight intensity={0.75} />
        <directionalLight position={[width * 0.3, 20, depth * 1.2]} intensity={1.4} />
        <directionalLight position={[width, 10, -depth]} intensity={0.4} />

        {/* Padló, rács (1 m-es cellák, 5 m-es fővonalak), körvonal */}
        <mesh rotation={[-Math.PI / 2, 0, 0]} position={[width / 2, 0, depth / 2]}>
          <planeGeometry args={[width, depth]} />
          <meshStandardMaterial color={palette.floor} roughness={1} />
        </mesh>
        <Grid
          position={[width / 2, 0.005, depth / 2]}
          args={[width, depth]}
          cellSize={1}
          sectionSize={5}
          cellColor={palette.gridCell}
          sectionColor={palette.gridSection}
          cellThickness={0.6}
          sectionThickness={1}
          fadeDistance={1000}
        />
        <Line points={outline} color={palette.wall} lineWidth={2} />

        {/* Depó: innen indul és ide érkezik minden felvétel / betárolás */}
        <mesh position={dockPosition}>
          <boxGeometry args={[2, 0.03, 1]} />
          <meshStandardMaterial color={palette.dock} />
        </mesh>
        <TextSprite text="Depó" position={[dockPosition[0], 0.6, dockPosition[2]]} {...label} {...labelSize} background={palette.dock} color="#212529" />

        {/* Polcok; felülnézetben a polclapok nélkül, különben eltakarnák a tárolóhelyeket */}
        {mode === '3d' && <InstancedBoxes boxes={rackBoxes.shelves} colors={palette.rack} />}
        <InstancedBoxes boxes={rackBoxes.posts} colors={palette.rack} />
        {layout.racks.map((rack) => (
          <TextSprite key={rack.id} text={rack.code} position={rackLabelPosition(rack, layout.racks)} {...label} {...labelSize} />
        ))}

        {/* Tárolóhelyek: foglalt (telítettség szerinti magasság és szín) és üres (áttetsző) */}
        <InstancedBoxes
          boxes={stockedBoxes}
          colors={stockedColors}
          onClick={(i) => onSelect(stocked[i]?.location.id ?? null)}
          onHover={(i) => setHovered(i === null ? null : stocked[i] ?? null)}
        />
        <InstancedBoxes
          boxes={emptyBoxes}
          colors={emptyColors}
          opacity={0.18}
          onClick={(i) => onSelect(empty[i]?.location.id ?? null)}
          onHover={(i) => setHovered(i === null ? null : empty[i] ?? null)}
        />
      </Canvas>
      {hovered && (
        <div className="layout3d-hover" aria-hidden="true">
          <strong>{hovered.location.code}</strong>
          {hovered.location.quantity} / {hovered.location.maxQuantity} db
        </div>
      )}
    </div>
  );
}
