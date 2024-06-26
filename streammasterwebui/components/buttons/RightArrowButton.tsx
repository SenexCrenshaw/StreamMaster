import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const RightArrowButton: React.FC<ChildButtonProperties> = ({ buttonDisabled = false, onClick, tooltip = 'Add' }) => (
  <SMButton buttonDisabled={buttonDisabled} icon="pi-chevron-right" iconFilled={false} onClick={onClick} tooltip={tooltip} />
);

export default RightArrowButton;
