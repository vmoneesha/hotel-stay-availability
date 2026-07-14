import { Routes } from '@angular/router';
import { reservationSelectionGuard } from './guards/reservation-selection.guard';
import { ConfirmationComponent } from './pages/confirmation/confirmation.component';
import { ReservationComponent } from './pages/reservation/reservation.component';
import { SearchComponent } from './pages/search/search.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'search' },
  { path: 'search', component: SearchComponent },
  { path: 'reservation', component: ReservationComponent, canActivate: [reservationSelectionGuard] },
  { path: 'confirmation/:reference', component: ConfirmationComponent },
  { path: '**', redirectTo: 'search' }
];