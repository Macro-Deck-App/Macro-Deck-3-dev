import { EdgeInsets } from '../models';

export function edgeInsetsToStyle(insets: EdgeInsets | undefined): string | undefined {
  if (!insets) return undefined;
  return `${insets.top}px ${insets.right}px ${insets.bottom}px ${insets.left}px`;
}

export function mapMainAxisAlignment(alignment: string | undefined): string {
  switch (alignment?.toLowerCase()) {
    case 'start': return 'flex-start';
    case 'center': return 'center';
    case 'end': return 'flex-end';
    case 'spacebetween': return 'space-between';
    case 'spacearound': return 'space-around';
    case 'spaceevenly': return 'space-evenly';
    default: return 'flex-start';
  }
}

export function mapCrossAxisAlignment(alignment: string | undefined, defaultValue = 'stretch'): string {
  switch (alignment?.toLowerCase()) {
    case 'start': return 'flex-start';
    case 'center': return 'center';
    case 'end': return 'flex-end';
    case 'stretch': return 'stretch';
    default: return defaultValue;
  }
}
