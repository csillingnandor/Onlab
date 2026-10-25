import type { Rack, StorageLocation } from '../../../util/Types.js';

// A raktár rácsa (1 cella = 1 m) és a 3D jelenet közti átváltás: a rács (x, y) cellája a jelenetben
// az X–Z síkon fekszik (x -> X, y -> Z), a polcszintek a Y tengelyen egymás fölött.
export const LEVEL_HEIGHT = 0.7; // egy polcszint magassága (m)
export const SHELF_THICKNESS = 0.04;
export const RACK_DEPTH = 0.9; // a polc mélysége a cellán belül
const BIN_FOOTPRINT = 0.78; // egy tárolóhely dobozának alapja
const BIN_MAX_HEIGHT = LEVEL_HEIGHT * 0.78;

export type Vec3 = [number, number, number];

// Egy doboz a jelenetben: középpont és méret (az InstancedBoxes ebből rajzol)
export type Box = {
  position: Vec3;
  size: Vec3;
};

const isHorizontal = (rack: Rack) => rack.orientation === 'Horizontal';

// A rekesz cellájának közepe a padlón (X, Z)
export function bayCenter(rack: Rack, bay: number): [number, number] {
  return isHorizontal(rack)
    ? [rack.x + bay - 0.5, rack.y + 0.5]
    : [rack.x + 0.5, rack.y + bay - 0.5];
}

export const rackHeight = (rack: Rack) => rack.levels * LEVEL_HEIGHT;

// A polc középpontja és kiterjedése a padlón; a felirat és a polclapok helyéhez
export function rackFootprint(rack: Rack): { center: [number, number]; size: [number, number] } {
  return isHorizontal(rack)
    ? { center: [rack.x + rack.length / 2, rack.y + 0.5], size: [rack.length, RACK_DEPTH] }
    : { center: [rack.x + 0.5, rack.y + rack.length / 2], size: [RACK_DEPTH, rack.length] };
}

// Polclapok: szintenként egy (alul) és egy tető; a polc teljes hosszában
export function shelfBoxes(rack: Rack): Box[] {
  const { center, size } = rackFootprint(rack);
  return Array.from({ length: rack.levels + 1 }, (_, i) => ({
    position: [center[0], i * LEVEL_HEIGHT + SHELF_THICKNESS / 2, center[1]],
    size: [size[0], SHELF_THICKNESS, size[1]],
  }));
}

// Oszlopok a rekeszhatárokon, a polc két hosszanti élén
export function postBoxes(rack: Rack): Box[] {
  const height = rackHeight(rack) + SHELF_THICKNESS;
  const edge = (1 - RACK_DEPTH) / 2;
  const boxes: Box[] = [];
  for (let i = 0; i <= rack.length; i++) {
    for (const side of [edge, 1 - edge]) {
      const [x, z] = isHorizontal(rack) ? [rack.x + i, rack.y + side] : [rack.x + side, rack.y + i];
      boxes.push({ position: [x, height / 2, z], size: [0.05, height, 0.05] });
    }
  }
  return boxes;
}

export const fillRatio = (location: StorageLocation) =>
  location.maxQuantity > 0 ? Math.min(1, location.quantity / location.maxQuantity) : 0;

// A tárolóhely doboza: a telítettséggel arányos magasságú (üres helynél teljes méretű "szellemdoboz")
export function locationBox(rack: Rack, location: StorageLocation): Box {
  const [x, z] = bayCenter(rack, location.bay);
  const base = (location.level - 1) * LEVEL_HEIGHT + SHELF_THICKNESS;
  const height = location.quantity > 0 ? BIN_MAX_HEIGHT * Math.max(0.2, fillRatio(location)) : BIN_MAX_HEIGHT;
  return { position: [x, base + height / 2, z], size: [BIN_FOOTPRINT, height, BIN_FOOTPRINT] };
}
