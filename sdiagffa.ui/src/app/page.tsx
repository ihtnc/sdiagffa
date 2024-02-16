"use client";

import { useState } from 'react'
import { Connection } from './api/api-client';
import Header from './components/header';
import CharacterPicker from './components/character-picker'
import ConnectionFinder from './components/connection-finder';
import ConnectionVisualiser from './components/connection-visualiser';
import Footer from './components/footer';

export default function Home() {
  const [ selectedReferenceId, setSelectedReferenceId ] = useState(0);
  const [ selectedTargetId, setSelectedTargetId ] = useState(0);
  const [ connectionResult, setConnectionResult ] = useState<Connection>(null);

  return (
    <main className="flex min-h-screen flex-col items-center px-24 gap-8">
      <section>
        <Header />
      </section>

      <section className="flex gap-8 flex-row">
        <CharacterPicker id="reference"
          onSelect={(item) => { setSelectedReferenceId(item.id); }} />

        <CharacterPicker id="target"
          onSelect={(item) => { setSelectedTargetId(item.id); }} />
      </section>

      <section>
        <ConnectionFinder
          referencePersonId={selectedReferenceId}
          targetPersonId={selectedTargetId}
          onSearchComplete={(result) => { setConnectionResult(result); }}
        />
      </section>

      <section className="w-9/12">
        <ConnectionVisualiser connection={connectionResult} />
      </section>

      <section>
        <Footer />
      </section>
    </main>
  );
}
