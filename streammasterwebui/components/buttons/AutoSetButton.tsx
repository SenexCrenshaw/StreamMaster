import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const AutoSetButton: React.FC<ChildButtonProperties> = ({ buttonDisabled = true, onClick, tooltip = 'Auto Set' }) => (
  <SMButton buttonDisabled={buttonDisabled} icon="pi-sort-numeric-up-alt" onClick={onClick} tooltip={tooltip} />
);

export default AutoSetButton;
