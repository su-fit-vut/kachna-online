import { NgModule } from "@angular/core";
import { DayLocPipe } from "./day-loc.pipe";
import { MonthLocPipe } from "./month-loc.pipe";
import { StateLocPipe } from "./state-loc.pipe";
import { TimeStrPipe } from "./time-str.pipe";
import { TimeLocativePipe } from './time-locative.pipe';

@NgModule({
  declarations: [
    DayLocPipe,
    MonthLocPipe,
    StateLocPipe,
    TimeStrPipe,
    TimeLocativePipe
  ],
  imports: [],
    exports: [
        DayLocPipe,
        MonthLocPipe,
        StateLocPipe,
        TimeStrPipe,
        TimeLocativePipe
    ]
})
export class PipesModule {
}
