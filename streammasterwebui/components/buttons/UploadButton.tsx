import BaseButton from './BaseButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const UploadButton: React.FC<ChildButtonProperties> = ({ className, disabled = false, label, outlined, onClick, tooltip = 'Upload' }) => {
  const iconFilled = label ? true : false;

  return (
    <BaseButton
      className={className}
      disabled={disabled}
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
