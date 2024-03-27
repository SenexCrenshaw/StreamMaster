import { LinkButton } from '@components/buttons/LinkButton';
import { memo } from 'react';

interface StreamCopyLinkDialogProperties {
  readonly realUrl?: string | undefined;
}

const StreamCopyLinkDialog = ({ realUrl }: StreamCopyLinkDialogProperties) => {
  return <LinkButton link={realUrl ?? ''} title="Stream Link" />;
};

StreamCopyLinkDialog.displayName = 'StreamCopyLinkDialog';

export default memo(StreamCopyLinkDialog);
