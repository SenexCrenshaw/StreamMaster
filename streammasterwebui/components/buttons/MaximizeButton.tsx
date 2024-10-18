import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const MaximizeButton: React.FC<ChildButtonProperties> = ({ iconFilled = true, onClick, tooltip = '' }) => (
  <SMButton
    buttonClassName="icon-blue"
    icon="pi-window-maximize"
    iconFilled={iconFilled}
    onClick={onClick}
    // rounded={iconFilled}
    // style={style}
    tooltip={tooltip}
  />
);

export default MaximizeButton;
