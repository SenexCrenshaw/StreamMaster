import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const SGAddButton: React.FC<ChildButtonProperties> = ({ buttonDisabled, onClick, tooltip }) => {
  return <SMButton buttonDisabled={buttonDisabled} iconFilled={false} icon="pi-circle" onClick={onClick} tooltip={tooltip} />;
};

export default SGAddButton;
