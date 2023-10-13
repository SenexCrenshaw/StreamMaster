import { type ChildButtonProps } from './BaseButton';
import BaseButton from './BaseButton';

const RefreshButton: React.FC<ChildButtonProps> = ({ disabled = false, onClick, tooltip = '' }) => {
  return <BaseButton disabled={disabled} icon="pi-sync" iconFilled={false} onClick={onClick} tooltip={tooltip} />;
};

export default RefreshButton;
