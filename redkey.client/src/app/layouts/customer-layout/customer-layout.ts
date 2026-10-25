import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SiteFooter } from '../../shared/components/site-footer/site-footer';
import { CustomerHeader } from './customer-header/customer-header';

/** Layout 2/4 — signed-in customer website (prototype: "الموقع الإلكتروني"). */
@Component({
  selector: 'app-customer-layout',
  imports: [RouterOutlet, CustomerHeader, SiteFooter],
  templateUrl: './customer-layout.html',
  styleUrl: './customer-layout.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CustomerLayout {}
