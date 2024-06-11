import { SMOverlayProperties } from './SMOverlayProperties';

export interface SMPopUpProperties extends SMOverlayProperties {
  onCloseClick?: () => void;
  onOkClick?: () => void;
  readonly children?: React.ReactNode;
  readonly closeButtonDisabled?: boolean;
  readonly disabled?: boolean;
  readonly okButtonDisabled?: boolean;
  readonly rememberKey?: string;
  readonly showRemember?: boolean;
}
