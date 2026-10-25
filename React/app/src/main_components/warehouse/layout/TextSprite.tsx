import { useEffect, useMemo } from 'react';
import { CanvasTexture, LinearFilter, SRGBColorSpace } from 'three';
import type { Vec3 } from './layoutGeometry.js';

type TextSpriteProps = {
  text: string;
  position: Vec3;
  height?: number; // a felirat magassága: méterben, vagy fixedSize esetén a képernyő magasságához mérten
  fixedSize?: boolean; // true: a kamera távolságától független, mindig azonos méretű (perspektivikus nézethez)
  color: string;
  background: string;
  border: string;
};

const FONT_SIZE = 64;
const PADDING = 18;

// Mindig a kamera felé forduló felirat. A szöveg egy 2D vászonra rajzolódik, abból lesz a textúra:
// nem kell hozzá betűtípust letölteni, és nem hoz létre külön DOM-elemet / React rootot sem.
export default function TextSprite({ text, position, height = 0.6, fixedSize = false, color, background, border }: TextSpriteProps) {
  const { texture, aspect } = useMemo(() => {
    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');
    const font = `700 ${FONT_SIZE}px system-ui, sans-serif`;
    if (ctx) ctx.font = font;
    const textWidth = ctx ? ctx.measureText(text).width : FONT_SIZE * text.length * 0.6;
    canvas.width = Math.ceil(textWidth + PADDING * 2);
    canvas.height = FONT_SIZE + PADDING * 2;

    if (ctx) {
      // A méret beállítása törli a vásznat és a betűtípust is
      ctx.fillStyle = background;
      ctx.strokeStyle = border;
      ctx.lineWidth = 6;
      ctx.beginPath();
      ctx.roundRect(3, 3, canvas.width - 6, canvas.height - 6, 16);
      ctx.fill();
      ctx.stroke();
      ctx.font = font;
      ctx.fillStyle = color;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      ctx.fillText(text, canvas.width / 2, canvas.height / 2 + 4);
    }

    const result = new CanvasTexture(canvas);
    result.colorSpace = SRGBColorSpace;
    result.minFilter = LinearFilter;
    return { texture: result, aspect: canvas.width / canvas.height };
  }, [text, color, background, border]);

  useEffect(() => () => texture.dispose(), [texture]);

  return (
    <sprite position={position} scale={[height * aspect, height, 1]} renderOrder={1}>
      <spriteMaterial map={texture} transparent depthTest={false} sizeAttenuation={!fixedSize} />
    </sprite>
  );
}
