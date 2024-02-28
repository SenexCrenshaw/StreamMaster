import BaseButton from './BaseButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const RefreshButton: React.FC<ChildButtonProperties> = ({ disabled = false, onClick, tooltip = '' }) => (
  <BaseButton disabled={disabled} icon="pi-sync" iconFilled={false} onClick={onClick} tooltip={tooltip} />
);

export default RefreshButton;
