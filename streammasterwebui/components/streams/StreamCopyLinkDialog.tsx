import { LinkButton } from '@components/buttons/LinkButton';
import { SMStreamDto } from '@lib/iptvApi';
import { memo } from 'react';

interface StreamCopyLinkDialogProperties {
  readonly iconFilled?: boolean | undefined;
  readonly onClose?: () => void;
  readonly value?: SMStreamDto | undefined;
}

const StreamCopyLinkDialog = ({ iconFilled, onClose, value }: StreamCopyLinkDialogProperties) => {
  return <LinkButton link={value?.realUrl ?? ''} title="Stream Link" />;
};

StreamCopyLinkDialog.displayName = 'StreamCopyLinkDialog';

export default memo(StreamCopyLinkDialog);
