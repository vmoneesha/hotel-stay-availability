import { Component, input } from '@angular/core';

interface BookingStep {
  readonly number: number;
  readonly title: string;
}

@Component({
  selector: 'app-booking-progress',
  templateUrl: './booking-progress.component.html',
  styleUrl: './booking-progress.component.scss'
})
export class BookingProgressComponent {
  readonly currentStep = input.required<number>();
  readonly steps: readonly BookingStep[] = [
    { number: 1, title: 'Your Selection' },
    { number: 2, title: 'Your Details' },
    { number: 3, title: 'Finish Booking' }
  ];

  stateFor(step: BookingStep): 'complete' | 'active' | 'upcoming' {
    if (step.number < this.currentStep()) {
      return 'complete';
    }

    return step.number === this.currentStep() ? 'active' : 'upcoming';
  }
}
