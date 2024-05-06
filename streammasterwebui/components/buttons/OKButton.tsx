import BaseButton from './BaseButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const OKButton: React.FC<ChildButtonProperties> = ({ disabled, iconFilled = true, label, style, onClick, tooltip = '' }) => (
  <BaseButton
    disabled={disabled}
    icon="pi-check"
    iconFilled={iconFilled}
    label={iconFilled === true ? (label === null ? undefined : 'Ok') : undefined}
    onClick={onClick}
    rounded={iconFilled}
    severity="success"
    style={style}
    tooltip={tooltip}
  />
);

export default OKButton;
