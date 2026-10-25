import { Pipe, PipeTransform } from '@angular/core';
import { Lang, Loc } from './i18n.models';

/** `{{ item.title | loc: lang() }}` — pure: only re-runs when the value or the language changes. */
@Pipe({ name: 'loc' })
export class LocPipe implements PipeTransform {
  transform(value: Loc | null | undefined, lang: Lang): string {
    return value ? value[lang] : '';
  }
}
