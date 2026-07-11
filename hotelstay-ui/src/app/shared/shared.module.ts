import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@NgModule({
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  exports: [CommonModule, ReactiveFormsModule, RouterLink]
})
export class SharedModule {}
