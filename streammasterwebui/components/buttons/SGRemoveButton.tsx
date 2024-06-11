import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const SGRemoveButton: React.FC<ChildButtonProperties> = ({ onClick, buttonDisabled, tooltip }) => {
  return <SMButton buttonDisabled={buttonDisabled} buttonClassName="icon-sg" icon="pi-check-circle" onClick={onClick} tooltip={tooltip} rounded />;
};

export default SGRemoveButton;
