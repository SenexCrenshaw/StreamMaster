import BaseButton, { type ChildButtonProps as ChildButtonProperties } from './BaseButton';

const DownArrowButton: React.FC<ChildButtonProperties> = ({ className, disabled = false, onClick, tooltip = 'Add Additional Channels' }) => (
  <BaseButton className={className} disabled={disabled} icon="pi-chevron-down" iconFilled={false} onClick={onClick} severity="success" tooltip={tooltip} />
);

export default DownArrowButton;
