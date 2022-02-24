import { BoardGameCategory } from "./board-game-category.model";
import { MadeByUser } from "../users/made-by-user.model";

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
  unavailable: number
  inStock: number = 0
  toReserve: number = 0
  category: BoardGameCategory
  noteInternal: string
  owner: MadeByUser
  visible: boolean
  defaultReservationDays: number | null
}
