import { NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { findOpportunity } from '../../../core/data/site.data';
import { I18nService } from '../../../core/i18n/i18n.service';
import { LocPipe } from '../../../core/i18n/loc.pipe';
import { MoneyPipe } from '../../../core/pipes/money.pipe';
import { Icon } from '../../../shared/components/icon/icon';
import { MY_INVESTMENTS } from '../account.data';

const ITEMS = MY_INVESTMENTS.flatMap(investment => {
  const opportunity = findOpportunity(investment.opportunityId);
  return opportunity ? [{ investment, opportunity }] : [];
});

const totalInvested = ITEMS.reduce((sum, i) => sum + i.investment.invested, 0);

const SUMMARY = {
  totalInvested,
  totalReturns: ITEMS.reduce((sum, i) => sum + i.investment.received, 0),
  shares: ITEMS.reduce((sum, i) => sum + i.investment.shares, 0),
  // Weighted by the invested amount.
  avgYield: totalInvested
    ? Math.round((ITEMS.reduce((sum, i) => sum + i.opportunity.yield * i.investment.invested, 0) / totalInvested) * 10) / 10
    : 0
};

@Component({
  selector: 'app-my-investments',
  imports: [TranslatePipe, NgOptimizedImage, RouterLink, Icon, LocPipe, MoneyPipe],
  templateUrl: './my-investments.html',
  styleUrl: './my-investments.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MyInvestments {
  protected readonly i18n = inject(I18nService);
  protected readonly items = ITEMS;
  protected readonly summary = SUMMARY;
}
