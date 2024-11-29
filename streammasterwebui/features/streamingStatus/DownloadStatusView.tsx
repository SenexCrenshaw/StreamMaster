import { DownloadStats } from '@lib/smAPI/smapiTypes';
import React from 'react';

interface DownloadStatusViewProps {
  stats: DownloadStats;
  title: string;
}

const DownloadStatusView = ({ stats, title }: DownloadStatusViewProps) => {
  const template = (name: string, value: number) => {
    return (
      <div className="sm-w-12 flex flex-row">
        <div className="sm-w-5">
          <div className="text-xs">{name}</div>
        </div>
        <div className="sm-w-4">
          <div className="text-xs">{value}</div>
        </div>
      </div>
    );
  };

  return (
    <>
      <div className="sm-border-right pl-2">
        <div className="header-text-sub">{title}</div>
        <div className="flex flex-row">
          <div className="sm-w-6 sm-border-right">
            {template('Queue', stats.Queue)}
            {template('Attempts', stats.Attempts)}
            {template('Successful', stats.Successful)}
          </div>
          <div className="sm-w-6 pl-2">
            {template('Exists', stats.AlreadyExists)}
            {template('Errors', stats.Errors)}
          </div>
        </div>
      </div>
    </>
  );
};

DownloadStatusView.displayName = 'Download Status View';

export default React.memo(DownloadStatusView);
