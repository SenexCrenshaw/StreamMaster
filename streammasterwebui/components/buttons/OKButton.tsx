import { type ChildButtonProps as ChildButtonProperties } from './BaseButton';
import BaseButton from './BaseButton';

const OKButton: React.FC<ChildButtonProperties> = ({ iconFilled = true, label, onClick, tooltip = '' }) => (
  <BaseButton
    icon="pi-check"
    iconFilled={iconFilled}
    label={iconFilled === true ? label || 'Ok' : undefined}
    onClick={onClick}
    rounded={iconFilled}
    severity="success"
    tooltip={tooltip}
  />
);

export default OKButton;
