import BaseButton, { type ChildButtonProps } from './BaseButton';

const XButton: React.FC<ChildButtonProps> = ({ disabled = false, onClick, tooltip = 'Remove', iconFilled, label }) => {
  return <BaseButton disabled={disabled} icon="pi-times" iconFilled={iconFilled} label={label} onClick={onClick} severity="danger" tooltip={tooltip} />;
};

export default XButton;
