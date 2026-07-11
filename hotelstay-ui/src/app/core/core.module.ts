import { NgModule, Optional, SkipSelf } from '@angular/core';

@NgModule({})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule | null) {
    if (parentModule) {
      throw new Error('CoreModule has already been loaded.');
    }
  }
}
