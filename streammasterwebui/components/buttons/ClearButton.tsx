import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const ClearButton: React.FC<ChildButtonProperties> = ({ buttonDisabled = true, onClick, tooltip = '' }) => (
  <SMButton buttonDisabled={buttonDisabled} icon="pi-book" onClick={onClick} tooltip={tooltip} />
);

export default ClearButton;
