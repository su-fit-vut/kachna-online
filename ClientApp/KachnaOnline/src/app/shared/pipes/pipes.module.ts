import { NgModule } from "@angular/core";
import { DayLocPipe } from "./day-loc.pipe";
import { MonthLocPipe } from "./month-loc.pipe";
import { StateLocPipe } from "./state-loc.pipe";
import { TimeStrPipe } from "./time-str.pipe";

@NgModule({
  declarations: [
    DayLocPipe,
    MonthLocPipe,
    StateLocPipe,
    TimeStrPipe
  ],
  imports: [],
  exports: [
    DayLocPipe,
    MonthLocPipe,
    StateLocPipe,
    TimeStrPipe
  ]
})
export class PipesModule {
}
