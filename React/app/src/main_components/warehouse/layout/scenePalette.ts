import { Color } from 'three';
import type { Theme } from '../../../theme/themeDom.js';

// A 3D jelenet színei témánként. A WebGL nem olvassa a CSS-változókat, ezért itt, a theme.css
// árnyalataihoz igazítva vannak megadva.
export type ScenePalette = {
  background: string;
  floor: string;
  gridCell: string;
  gridSection: string;
  wall: string;
  dock: string;
  rack: string;
  emptyBin: string;
  fillLow: string; // alig foglalt hely
  fillHigh: string; // tele
  highlight: string; // a kiválasztott termék helyei
  selected: string;
  labelText: string;
  labelBackground: string;
  labelBorder: string;
};

const palettes: Record<Theme, ScenePalette> = {
  light: {
    background: '#f1f3f5',
    floor: '#e2e6ea',
    gridCell: '#cfd4da',
    gridSection: '#adb5bd',
    wall: '#495057',
    dock: '#ffc107',
    rack: '#6c757d',
    emptyBin: '#adb5bd',
    fillLow: '#9ec5fe',
    fillHigh: '#0a58ca',
    highlight: '#fd7e14',
    selected: '#d63384',
    labelText: '#212529',
    labelBackground: '#ffffff',
    labelBorder: '#adb5bd',
  },
  dark: {
    background: '#1a1d20',
    floor: '#24282d',
    gridCell: '#30353b',
    gridSection: '#495057',
    wall: '#adb5bd',
    dock: '#ffda6a',
    rack: '#868e96',
    emptyBin: '#6c757d',
    fillLow: '#3d6aa8',
    fillHigh: '#9ec5fe',
    highlight: '#fd9843',
    selected: '#e685b5',
    labelText: '#f8f9fa',
    labelBackground: '#212529',
    labelBorder: '#6c757d',
  },
};

export const scenePalette = (theme: Theme) => palettes[theme];

// Telítettség (0..1) szerinti szín a két végpont között
export function fillColor(palette: ScenePalette, ratio: number): Color {
  return new Color(palette.fillLow).lerp(new Color(palette.fillHigh), Math.min(1, Math.max(0, ratio)));
}
