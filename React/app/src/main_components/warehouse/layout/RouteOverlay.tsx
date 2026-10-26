import { useMemo, useRef } from 'react';
import { useFrame } from '@react-three/fiber';
import { Line } from '@react-three/drei';
import type { Mesh } from 'three';
import type { PickRoute } from '../../../util/Types.js';
import TextSprite from './TextSprite.js';
import type { Vec3 } from './layoutGeometry.js';
import type { ScenePalette } from './scenePalette.js';

type RouteOverlayProps = {
  route: PickRoute;
  palette: ScenePalette;
  playing: boolean;
  fixedSizeLabels: boolean;
};

const PATH_HEIGHT = 0.06; // a padló fölött, hogy ne villogjon össze a ráccsal
const SPEED = 3; // m/s (a valóságnál gyorsabb, hogy a bemutató ne legyen hosszú)
const PAUSE = 1.2; // s, a depóban az újabb kör előtt

// A rács cellájának közepe a padló fölött
const cellCenter = (x: number, y: number, height = PATH_HEIGHT): Vec3 => [x + 0.5, height, y + 0.5];

// A felvételi körút a 3D jelenetben: vonal a cellák közepén, sorszámozott megállók, és egy "targonca",
// amely állandó sebességgel végigmegy az útvonalon, majd a depóban rövid szünet után újrakezdi.
export default function RouteOverlay({ route, palette, playing, fixedSizeLabels }: RouteOverlayProps) {
  const picker = useRef<Mesh>(null);
  const elapsed = useRef(0);

  const points = useMemo(() => route.path.map((p) => cellCenter(p.x, p.y)), [route]);
  const length = points.length - 1; // szomszédos cellák: minden szakasz 1 m

  useFrame((_, delta) => {
    const mesh = picker.current;
    if (!mesh || length <= 0) return;
    if (playing) elapsed.current += delta;

    const travelled = Math.min((elapsed.current % (length / SPEED + PAUSE)) * SPEED, length);
    const index = Math.min(Math.floor(travelled), length - 1);
    const t = travelled - index;
    const from = points[index];
    const to = points[index + 1];
    if (!from || !to) return;
    mesh.position.set(from[0] + (to[0] - from[0]) * t, 0.35, from[2] + (to[2] - from[2]) * t);
  });

  const labelSize = fixedSizeLabels ? { fixedSize: true, height: 0.04 } : { height: 0.6 };

  return (
    <>
      {points.length > 1 && <Line points={points} color={palette.route} lineWidth={4} />}

      {route.stops.map((stop) => (
        <group key={stop.sequence}>
          <mesh position={cellCenter(stop.x, stop.y, 0.04)} rotation={[-Math.PI / 2, 0, 0]}>
            <circleGeometry args={[0.35, 24]} />
            <meshBasicMaterial color={palette.route} />
          </mesh>
          <TextSprite
            text={String(stop.sequence)}
            position={cellCenter(stop.x, stop.y, 0.9)}
            color="#ffffff"
            background={palette.route}
            border={palette.route}
            {...labelSize}
          />
        </group>
      ))}

      {length > 0 && (
        <mesh ref={picker} position={points[0] ?? [0, 0, 0]}>
          <capsuleGeometry args={[0.22, 0.3, 6, 12]} />
          <meshStandardMaterial color={palette.picker} emissive={palette.picker} emissiveIntensity={0.35} />
        </mesh>
      )}
    </>
  );
}
