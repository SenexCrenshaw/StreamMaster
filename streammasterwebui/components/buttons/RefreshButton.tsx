import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const RefreshButton: React.FC<ChildButtonProperties> = ({ disabled = false, onClick, tooltip = '' }) => (
  <SMButton disabled={disabled} icon="pi-sync" iconFilled={false} onClick={onClick} tooltip={tooltip} />
);

export default RefreshButton;
