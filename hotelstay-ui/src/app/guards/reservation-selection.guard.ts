import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { HotelSelectionStore } from '../services/hotel-selection.store';

export const reservationSelectionGuard: CanActivateFn = () => {
  const selectionStore = inject(HotelSelectionStore);
  const router = inject(Router);

  return selectionStore.current() ? true : router.createUrlTree(['/search']);
};