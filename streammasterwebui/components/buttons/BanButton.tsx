import BaseButton, { type ChildButtonProps as ChildButtonProperties } from './BaseButton';

const BanButton: React.FC<ChildButtonProperties> = ({ className, disabled = false, onClick, tooltip = '' }) => (
  <BaseButton className={className} disabled={disabled} icon="pi-ban" iconFilled={false} onClick={onClick} tooltip={tooltip} />
);

export default BanButton;
