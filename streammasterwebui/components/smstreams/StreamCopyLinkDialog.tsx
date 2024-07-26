import { LinkButton } from '@components/buttons/LinkButton';
import { memo } from 'react';

interface StreamCopyLinkDialogProperties {
  readonly realUrl?: string | undefined;
  readonly toolTip?: string | undefined;
}

const StreamCopyLinkDialog = ({ realUrl, toolTip = 'Stream Link' }: StreamCopyLinkDialogProperties) => {
  return <LinkButton link={realUrl ?? ''} title={toolTip} />;
};

StreamCopyLinkDialog.displayName = 'StreamCopyLinkDialog';

export default memo(StreamCopyLinkDialog);
