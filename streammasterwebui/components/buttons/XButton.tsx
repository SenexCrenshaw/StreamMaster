import BaseButton, { type ChildButtonProps as ChildButtonProperties } from './BaseButton';

const XButton: React.FC<ChildButtonProperties> = ({ disabled = false, onClick, tooltip = 'Remove', iconFilled, label }) => <BaseButton disabled={disabled} icon="pi-times" iconFilled={iconFilled} label={label} onClick={onClick} severity="danger" tooltip={tooltip} />;

export default XButton;
