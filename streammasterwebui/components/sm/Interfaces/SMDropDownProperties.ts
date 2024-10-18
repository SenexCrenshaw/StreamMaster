import { SMOverlayProperties } from './SMOverlayProperties';
import { SMScrollerProperties } from './SMScrollerProperties';

export interface SMDropDownProperties extends SMOverlayProperties, SMScrollerProperties {
  readonly closeOnSelection?: boolean;
  readonly footerTemplate?: React.ReactNode;
  readonly labelInline?: boolean;
  readonly children?: React.ReactNode;
}
