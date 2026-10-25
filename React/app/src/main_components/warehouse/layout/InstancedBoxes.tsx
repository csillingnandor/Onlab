import { useLayoutEffect, useRef } from 'react';
import { Color, Object3D, type InstancedMesh } from 'three';
import type { ThreeEvent } from '@react-three/fiber';
import type { Box } from './layoutGeometry.js';

type InstancedBoxesProps = {
  boxes: Box[];
  // Dobozonként (a boxes sorrendjében) vagy egyetlen közös szín
  colors: Color[] | string;
  opacity?: number;
  onClick?: ((index: number) => void) | undefined;
  onHover?: ((index: number | null) => void) | undefined;
};

const dummy = new Object3D();

// Sok azonos alakú doboz egyetlen rajzolási hívással (InstancedMesh): a polcok, oszlopok és a több száz
// tárolóhely így sem lassítja a jelenetet. A kattintás / hover a doboz indexét adja vissza.
export default function InstancedBoxes({ boxes, colors, opacity = 1, onClick, onHover }: InstancedBoxesProps) {
  const ref = useRef<InstancedMesh>(null);

  useLayoutEffect(() => {
    const mesh = ref.current;
    if (!mesh) return;

    boxes.forEach((box, i) => {
      dummy.position.set(...box.position);
      dummy.scale.set(...box.size);
      dummy.updateMatrix();
      mesh.setMatrixAt(i, dummy.matrix);
      if (Array.isArray(colors)) {
        const color = colors[i];
        if (color) mesh.setColorAt(i, color);
      }
    });
    mesh.instanceMatrix.needsUpdate = true;
    if (mesh.instanceColor) mesh.instanceColor.needsUpdate = true;
    // A kattintás sugárkövetése a befoglaló gömbbel szűr, ezért a dobozok helye után újra kell számolni
    mesh.computeBoundingSphere();
  }, [boxes, colors]);

  if (boxes.length === 0) return null;

  // Eseménykezelő csak az interaktív dobozokon: a többin a sugárkövetés is kimarad
  const clickHandlers = onClick && {
    onClick: (event: ThreeEvent<MouseEvent>) => {
      // A kamera forgatása / húzása (elmozdult egér) nem számít kattintásnak
      if (event.instanceId === undefined || event.delta > 4) return;
      event.stopPropagation();
      onClick(event.instanceId);
    },
  };
  const hoverHandlers = onHover && {
    onPointerMove: (event: ThreeEvent<PointerEvent>) => {
      if (event.instanceId === undefined) return;
      event.stopPropagation();
      onHover(event.instanceId);
    },
    onPointerOut: () => onHover(null),
  };

  return (
    <instancedMesh
      // A darabszám az InstancedMesh létrehozásakor dől el, ezért eltérő számnál új példány kell
      key={boxes.length}
      ref={ref}
      args={[undefined, undefined, boxes.length]}
      {...clickHandlers}
      {...hoverHandlers}
    >
      <boxGeometry args={[1, 1, 1]} />
      <meshStandardMaterial
        color={Array.isArray(colors) ? '#ffffff' : colors}
        transparent={opacity < 1}
        opacity={opacity}
        depthWrite={opacity === 1}
        roughness={0.8}
      />
    </instancedMesh>
  );
}
