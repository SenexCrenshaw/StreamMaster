'use client';

import dynamic from 'next/dynamic';

const SettingsEditor = dynamic(() => import('@/features/settings/SettingsEditor'), { ssr: false });

export default function SettingsLayout() {
  return <SettingsEditor />;
}
