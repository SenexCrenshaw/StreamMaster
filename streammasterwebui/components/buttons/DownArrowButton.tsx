import BaseButton, { type ChildButtonProps } from './BaseButton';

const DownArrowButton: React.FC<ChildButtonProps> = ({ className, disabled = false, onClick, tooltip = 'Add Additional Channels' }) => {
  return (
    <BaseButton className={className} disabled={disabled} icon="pi-chevron-down" iconFilled={false} onClick={onClick} severity="success" tooltip={tooltip} />
  );
};

export default DownArrowButton;
