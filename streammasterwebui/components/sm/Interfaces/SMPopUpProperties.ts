import { SMOverlayProperties } from './SMOverlayProperties';

export interface SMPopUpProperties extends SMOverlayProperties {
  readonly children?: React.ReactNode;
  readonly disabled?: boolean;
  readonly isPopupLoading?: boolean;
  // readonly rememberKey?: string;
  // readonly showRemember?: boolean;
}
