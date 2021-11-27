// made-by-user-model.ts
// Author: František Nečas

/**
 * Contains basic identification about a user.
 *
 * This is returned from various parts of the API (e.g. assignee of a reservation,
 * creator of a state).
 */
export class MadeByUser {
  name: string
  discordId: string
}
