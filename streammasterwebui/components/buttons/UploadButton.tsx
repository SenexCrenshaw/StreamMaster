import BaseButton from './BaseButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const UploadButton: React.FC<ChildButtonProperties> = ({ disabled = false, label, outlined, onClick, tooltip = 'Delete Stream' }) => (
  <BaseButton
    className="p-1 px-2 text-xs col-3"
    disabled={disabled}
    icon="pi-upload"
    iconFilled={label ? true : false}
    onClick={onClick}
    label={label ?? undefined}
    outlined={outlined}
    severity="danger"
    tooltip={tooltip}
  />
);

export default UploadButton;
