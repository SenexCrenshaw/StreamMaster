import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const CloseButton: React.FC<ChildButtonProperties> = ({ onClick }) => {
  return <SMButton outlined className="icon-blue" icon="pi-times" iconFilled={false} onClick={onClick} tooltip="Close" />;
};

export default CloseButton;
