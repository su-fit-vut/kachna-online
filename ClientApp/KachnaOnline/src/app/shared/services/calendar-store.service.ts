// calendar-store.service.ts
// Author: František Nečas

import { Injectable } from '@angular/core';

/**
 * Used to temporarily store information about the state of the homepage calendar.
 */
@Injectable({
  providedIn: 'root'
})
export class CalendarStoreService {
  currentDate: Date

  constructor() { }

  /**
   * Returns the previously stored date.
   */
  getDate(): Date {
    return this.currentDate;
  }

  /**
   * Stores the current calendar date.
   * @param date The date to save.
   */
  setDate(date: Date) {
    this.currentDate = date;
  }
}
