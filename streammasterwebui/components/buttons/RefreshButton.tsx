import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const RefreshButton: React.FC<ChildButtonProperties> = ({ buttonDisabled = false, onClick, tooltip = '' }) => (
  <SMButton buttonDisabled={buttonDisabled} icon="pi-sync" iconFilled={false} onClick={onClick} tooltip={tooltip} />
);

export default RefreshButton;
