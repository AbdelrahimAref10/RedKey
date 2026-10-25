import { ChangeDetectionStrategy, Component } from '@angular/core';
import { AreasSection } from './sections/areas-section/areas-section';
import { BrokerCta } from './sections/broker-cta/broker-cta';
import { FeaturedProperties } from './sections/featured-properties/featured-properties';
import { HeroSearch } from './sections/hero-search/hero-search';
import { InvestBanner } from './sections/invest-banner/invest-banner';
import { PropertyTypes } from './sections/property-types/property-types';
import { TestimonialsSection } from './sections/testimonials-section/testimonials-section';
import { WhyUs } from './sections/why-us/why-us';

/** Landing page — every section below the fold is deferred until it scrolls into view. */
@Component({
  selector: 'app-landing-home',
  imports: [
    HeroSearch,
    PropertyTypes,
    FeaturedProperties,
    WhyUs,
    AreasSection,
    InvestBanner,
    TestimonialsSection,
    BrokerCta
  ],
  templateUrl: './landing-home.html',
  styleUrl: './landing-home.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LandingHome {}
