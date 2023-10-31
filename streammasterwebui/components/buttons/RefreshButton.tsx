import { type ChildButtonProps as ChildButtonProperties } from './BaseButton';
import BaseButton from './BaseButton';

const RefreshButton: React.FC<ChildButtonProperties> = ({ disabled = false, onClick, tooltip = '' }) => (
  <BaseButton disabled={disabled} icon="pi-sync" iconFilled={false} onClick={onClick} tooltip={tooltip} />
);

export default RefreshButton;
