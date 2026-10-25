import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SiteFooter } from '../../shared/components/site-footer/site-footer';
import { LandingHeader } from './landing-header/landing-header';

/** Layout 1/4 — public website (landing, properties, invest, contact, login, register). */
@Component({
  selector: 'app-landing-layout',
  imports: [RouterOutlet, LandingHeader, SiteFooter],
  templateUrl: './landing-layout.html',
  styleUrl: './landing-layout.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LandingLayout {}
