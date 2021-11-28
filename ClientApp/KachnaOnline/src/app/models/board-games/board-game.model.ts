// board-game.model.ts
// Author: František Nečas

import { BoardGameCategory } from "./board-game-category.model";

/**
 * Model of a board game as returned by the backend to a regular user.
 */
export class BoardGame {
  id: number
  name: string = ""
  description: string = ""
  imageUrl: string = ""
  playersMin: number
  playersMax: number
  available: number = 0
  inStock: number = 0
  toReserve: number = 0
  category: BoardGameCategory
}
