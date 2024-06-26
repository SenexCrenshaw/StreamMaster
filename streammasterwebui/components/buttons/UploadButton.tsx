import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const UploadButton: React.FC<ChildButtonProperties> = ({ buttonClassName, buttonDisabled = false, label, outlined, onClick, tooltip = 'Upload' }) => {
  const iconFilled = label ? true : false;

  return (
    <SMButton
      buttonClassName={buttonClassName}
      buttonDisabled={buttonDisabled}
      icon="pi-upload"
      iconFilled={iconFilled}
      onClick={onClick}
      label={label ?? undefined}
      outlined={outlined}
      tooltip={tooltip}
    />
  );
};

export default UploadButton;
