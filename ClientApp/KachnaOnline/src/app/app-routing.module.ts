import { HomeComponent } from './home/home.component';
import { PageNotFoundComponent } from "./page-not-found/page-not-found.component";
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ForbiddenComponent } from "./forbidden/forbidden.component";

const routes: Routes = [
  /* Route order
The order of routes is important because the Router uses a first-match wins strategy when matching routes, so more specific routes should be placed above less specific routes. List routes with a static path first, followed by an empty path route, which matches the default route. The wildcard route comes last because it matches every URL and the Router selects it only if no other routes match first.
*/
  {path: 'forbidden', component: ForbiddenComponent},
  {path: 'home', redirectTo: '', pathMatch: 'full'}, // Redirect from '/home' to default home page.
  {path: '', component: HomeComponent}, // Default home page.
  {path: '**', component: PageNotFoundComponent},  // Wildcard route for a 404 page.
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {onSameUrlNavigation: "reload"})],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
