import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const DownArrowButton: React.FC<ChildButtonProperties> = ({ buttonClassName, buttonDisabled = false, label, onClick, tooltip = 'Add Additional Channels' }) => (
  <SMButton
    buttonClassName={buttonClassName}
    buttonDisabled={buttonDisabled}
    icon="pi-chevron-down"
    iconFilled={false}
    label={label ?? ''}
    onClick={onClick}
    tooltip={tooltip}
  />
);

export default DownArrowButton;
