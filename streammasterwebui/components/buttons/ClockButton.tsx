import BaseButton, { type ChildButtonProps } from './BaseButton';

const ClockButton: React.FC<ChildButtonProps> = ({ disabled = false, label, onClick, tooltip = 'Time Shift', iconFilled }) => {
  return <BaseButton disabled={disabled} icon="pi-stopwatch" iconFilled={iconFilled} label={label} onClick={onClick} tooltip={tooltip} />;
};

export default ClockButton;
