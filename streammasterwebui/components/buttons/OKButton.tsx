import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const OKButton: React.FC<ChildButtonProperties> = ({ buttonDisabled, iconFilled = true, label, onClick, tooltip = '' }) => (
  <SMButton
    buttonClassName="icon-green"
    buttonDisabled={buttonDisabled}
    icon="pi-check"
    iconFilled={iconFilled}
    label={label ?? undefined}
    onClick={onClick}
    // rounded={iconFilled}
    // style={style}
    tooltip={tooltip}
  />
);

export default OKButton;
